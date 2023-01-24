using System.Collections.Concurrent;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Features.Metadata;
using Autofac.Features.ResolveAnything;

namespace DI_Playground
{
    
    public class Program
    {
        public interface ICommand
        {
            void Execute();
        }

        class SaveCommand : ICommand
        {
            public void Execute()
            {
                Console.WriteLine("Saving a file");
            }
        }
        
        class OpenCommand : ICommand
        {
            public void Execute()
            {
                Console.WriteLine("Opening a file");
            }
        }
        
        public class Button
        {
            private ICommand _command;
            private string _name;

            public Button(ICommand command, string name)
            {
                _command = command;
                _name = name;
            }

            public void Click()
            {
                _command.Execute();
            }

            public void PrintMe()
            {
                Console.WriteLine($"I am a button called {_name}");
            }
        }

        public class Editor
        {
            private IEnumerable<Button> _buttons;

            public IEnumerable<Button> Buttons => _buttons;

            public Editor(IEnumerable<Button> buttons)
            {
                _buttons = buttons;
            }

            public void ClickAll()
            {
                foreach (var button in _buttons)
                {
                    button.Click();
                }
            }
        }
        
        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<SaveCommand>().As<ICommand>()
                .WithMetadata("_name","save");
            b.RegisterType<OpenCommand>().As<ICommand>()
                .WithMetadata("_name","open");
            //b.RegisterType<Button>();
            //b.RegisterAdapter<ICommand, Button>(cmd => new Button(cmd));
            b.RegisterAdapter<Meta<ICommand>, Button>(cmd =>
                new Button(cmd.Value, (string)cmd.Metadata["_name"]));
            b.RegisterType<Editor>();

            using (var c = b.Build())
            {
                var editor = c.Resolve<Editor>();
                //editor.ClickAll();

                foreach (var btn in editor.Buttons)
                {
                    btn.PrintMe();
                }
            }
        }
    }
}