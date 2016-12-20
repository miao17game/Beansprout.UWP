using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;

namespace Wallace.UWP.Helpers. Tools {
    /// <summary>
    /// 定义一个命令委托类，继承ICommand接口，重写相应的接口函数
    /// </summary>
    public sealed class DelegateCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// 需要手动触发属性改变事件
        /// https://msdn.microsoft.com/en-us/magazine/5eafc6fc-713a-4461-bc2b-469afdd03c31
        /// http://www.mvvmlight.net/help/
        /// </summary>
        public void RaiseCanExecuteChanged ( ) {
            CanExecuteChanged?.Invoke ( this , EventArgs . Empty ); }

        /// <summary>
        /// 决定当前绑定的Command能否被执行
        /// true：可以被执行
        /// false：不能被执行
        /// </summary>
        /// <param name="parameter">不是必须的，可以依据情况来决定，或者重写一个对应的无参函数</param>
        /// <returns></returns>
        public bool CanExecute ( object parameter ) {
            return MyCanExecute == null ? true : MyCanExecute ( parameter );
        }

        /// <summary>
        /// 用于执行对应的命令，只有在CanExecute可以返回true的情况下才可以被执行
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute ( object parameter ) {
            try {
                MyExecute ( parameter );
            } catch ( Exception ex ) { Debug . WriteLine ( ex . Message ); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Action<object> MyExecute { get; set; }
        public Func<object , bool> MyCanExecute { get; set; }

        /// <summary>
        /// 构造函数，用于初始化
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public DelegateCommand ( Action<object> execute , Func<object , bool> canExecute ) {
            MyExecute = execute;
            MyCanExecute = canExecute;
        }
    }
}
