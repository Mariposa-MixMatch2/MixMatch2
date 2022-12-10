using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MixMatch2.Shared.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Call to notify that a property has changed in the ViewModel.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper function to call OnPropertyChanged cleanly.
        /// </summary>
        /// <typeparam name="T"> The type of the value to set. </typeparam>
        /// <param name="field">A reference to the field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The property name. Optional, as it will use the CallerMemberName if not called. </param>
        /// <returns>True if a value changed, false if the value didn't change. </returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
