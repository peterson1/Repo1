using System;
using System.Threading.Tasks;

namespace Repo1.WPF45.SDK.InputCommands
{
    public class R1Command
    {
        public static R1AsyncCommandWPF Async(Func<Task> task, Predicate<object> canExecute = null, string buttonLabel = null)
            => new R1AsyncCommandWPF(task, canExecute, buttonLabel);


        //public static R1RelayCommandWPF Relay(Action action, Func<object, bool> canExecute = null)
        //    => new R1RelayCommandWPF(x => action(), canExecute);


        public static R1AsyncCommandWPF Relay(Action action, Predicate<object> canExecute = null, string buttonLabel = null)
            => new R1AsyncCommandWPF(async () => 
            {
                await Task.Delay(1);
                action?.Invoke();
            }, 
            canExecute, buttonLabel);
    }
}
