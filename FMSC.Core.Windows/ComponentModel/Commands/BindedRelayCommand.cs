using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace FMSC.Core.Windows.ComponentModel.Commands
{
    public class BindedRelayCommand<TViewModel> : RelayCommand, IDisposable where TViewModel : class, INotifyPropertyChanged
    {
        private TViewModel _Model;
        private List<string> _WatchedProperties;

        /// <summary>
        /// RelayCommand that binds to one or more properties of a model
        /// </summary>
        /// <param name="execute">Execution Action</param>
        /// <param name="canExecute">Predicate for when command can be executed</param>
        /// <param name="model">Model to bind to</param>
        /// <param name="propertiesToWatch">Properties to watch in the model, null watched all properties</param>
        public BindedRelayCommand(Action<object> execute, Predicate<object> canExecute,
            TViewModel model, Expression<Func<TViewModel, object>> propertiesToWatch = null)
           : base(execute, canExecute)
        {
            RegisterPropertiesWatcher(propertiesToWatch);
            (_Model = model).PropertyChanged += PropertyChangedHandler;
        }

        ~BindedRelayCommand()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_Model != null)
                _Model.PropertyChanged -= PropertyChangedHandler;
            _Model = null;
            _CanExecute = null;
            _Execute = null;
        }


        /// <summary>
        /// handler that, everytime a monitored property changes, calls the RaiseCanExecuteChanged of the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_WatchedProperties.Contains(e.PropertyName))
                this.OnCanExecuteChanged(sender, e);
        }

        /// <summary>
        /// giving an expression that identify a propriety or a list of properties, return the property names obtained from the expression
        /// Examples on selector usage
        /// Single:
        ///   entity => entity.PropertyName
        ///Multiple:
        ///   entity => new { entity.PropertyName1, entity.PropertyName2 }
        /// </summary>
        /// <param name="propertiesToWatch"></param>
        /// <returns></returns>
        protected void RegisterPropertiesWatcher(Expression<Func<TViewModel, object>> propertiesToWatch)
        {
            _WatchedProperties = new List<string>();

            if (propertiesToWatch == null)
            {
                _WatchedProperties = typeof(TViewModel).GetProperties().Select(p => p.Name).ToList();
            }
            else if (propertiesToWatch.Body is MemberExpression)
            {
                _WatchedProperties.Add(((MemberExpression)(propertiesToWatch.Body)).Member.Name);
            }
            else if (propertiesToWatch.Body is UnaryExpression)
            {
                _WatchedProperties.Add(((MemberExpression)(((UnaryExpression)(propertiesToWatch.Body)).Operand)).Member.Name);
            }
            else if (propertiesToWatch.Body.NodeType == ExpressionType.New)
            {
                foreach (var argument in ((NewExpression)propertiesToWatch.Body).Arguments)
                {
                    if (argument is MemberExpression)
                    {
                        _WatchedProperties.Add(((MemberExpression)argument).Member.Name);
                    }
                    else
                    {
                        throw new SyntaxErrorException();
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException();
            }
        }
    }
}
