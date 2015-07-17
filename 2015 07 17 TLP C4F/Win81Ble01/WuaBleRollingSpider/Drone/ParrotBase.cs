using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WuaBleRollingSpider.Annotations;
using WuaBleRollingSpider.Model;

namespace WuaBleRollingSpider.Drone
{
    public class ParrotBase : INotifyPropertyChanged
    {
        public ParrotBase()
        {
            _logActions = new List<LogAction>();
            _logExceptions = new List<LogAction>();
        }

        public void AddLogAction(string action)
        {
            LogActions.Add(new LogAction { ActionData = action });
        }

        private List<LogAction> _logActions;

        public List<LogAction> LogActions
        {
            get { return _logActions; }
            set
            {
                if (Equals(value, _logActions)) return;
                _logActions = value;
                OnPropertyChanged();
            }
        }

        private List<LogAction> _logExceptions;

        public List<LogAction> LogExceptions
        {
            get { return _logExceptions; }
            set
            {
                if (Equals(value, _logExceptions)) return;
                _logExceptions= value;
                OnPropertyChanged();
            }
        }

        public void AddException(Exception exception)
        {
            _logExceptions.Add(new LogAction {ActionData = exception.ToString()});
        }

        public void ClearExceptions()
        {
            _logExceptions.Clear();
        }

        public string GetExceptionStack()
        {
            var exceptionStack = $"  -- Exception {Environment.NewLine}";
            foreach (var logException in _logExceptions)
            {
                exceptionStack += $"{logException.ActionData}{Environment.NewLine}";
            }
            return exceptionStack;
        }

        public bool HasExceptionStack => LogExceptions.Count > 0;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
