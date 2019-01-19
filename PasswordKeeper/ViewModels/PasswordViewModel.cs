using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PasswordKeeper.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PasswordKeeper.ViewModels
{
    public class PasswordViewModel : ViewModelBase
    {
        private PasswordNoteAccess _passwordNoteAccess;

        public PasswordViewModel()
        {
            _passwordNoteAccess = new PasswordNoteAccess();
        }

        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((e) =>
                {
                    var result = MessageBox.Show("Вы действительно хотите закрыть программу?", "Выход", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                });
            }
        }


        #region Работа с данными. 

        bool isInsertMode = false;
        public RelayCommand<object> RowEditEndingCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    PasswordNoteModel password = new PasswordNoteModel();
                    PasswordNoteModel emp = obj as PasswordNoteModel;

                    password.Title = emp.Title;
                    password.Login = emp.Login;
                    password.Password = emp.Password;

                    if (isInsertMode)
                    {
                        password.Id = _passwordNoteAccess.Id;
                        _passwordNoteAccess.AddNote(password);
                        isInsertMode = false;
                    }
                    else
                    {
                        password.Id = emp.Id;
                        _passwordNoteAccess.UpdateNote(password);
                    }

                    RaisePropertyChanged(() => SourceItems);
                });
            }
        }

        public RelayCommand AddingNewItemCommand
        {
            get
            {
                return new RelayCommand(() => isInsertMode = true);
                
            }
        }

        public RelayCommand<Collection<object>> DeleteRowCommand
        {
            get
            {
                return new RelayCommand<Collection<object>>((selectedItems) =>
                {
                    try
                    {
                        var list = selectedItems.Cast<PasswordNoteModel>().ToList();

                        string noteWord = list.Count > 1 ? list.Count >= 5 ? "записей" : "записи" : "запись";
                        string deleteWord = list.Count > 1 ? "удалены" : "удалена";

                        var result = MessageBox.Show($"Вы уверены что хотите удалить {list.Count} {noteWord}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (result == MessageBoxResult.Yes)
                        {               
                            _passwordNoteAccess.DeleteNotes(list);
                            MessageBox.Show($"{list.Count} {noteWord} {deleteWord}.", "Запись удалена");
                            RaisePropertyChanged(() => SourceItems);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Снимите выделение с последней строки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        private ObservableCollection<PasswordNoteModel> GetData
        {
            get
            {
                return new ObservableCollection<PasswordNoteModel>(_passwordNoteAccess.GetData());
            }
        }

        #endregion


        #region Фильтр

        public ICollectionView SourceItems
        {
            get
            {
                if (Filter == string.Empty)
                {
                    return CollectionViewSource.GetDefaultView(GetData);
                }
                else
                {
                    return CollectionViewSource.GetDefaultView(new ObservableCollection<PasswordNoteModel>(from name in GetData where name.Title.ToLower().Contains(filter) select name));
                }
            }
        }

        private string filter = string.Empty;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                RaisePropertyChanged(nameof(SourceItems));
            }
        }

        #endregion
    }
}
