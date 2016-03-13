using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
         {
             PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
             if (propertyChanged != null)
             {
                 string propertyName = GetPropertyName(propertyExpression);
                 propertyChanged(this, new PropertyChangedEventArgs(propertyName));
             }
        }
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }
            return propertyInfo.Name;
        }


    }

}
