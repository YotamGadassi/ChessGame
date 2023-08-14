using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Common.Interfaces
{
    public interface IFramework
    {
        DependencyObject ViewModel { get; }
        Control          View      { get; }

        void Init();
        void Close();
    }
}
