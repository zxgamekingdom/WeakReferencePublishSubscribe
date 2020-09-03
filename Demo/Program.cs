using System;
using System.Threading.Tasks;
using WeakReferencePublishSubscribe;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Run();
            while (true)
            {
                GC.Collect();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static async Task Run()
        {
            for (int i = 0; i < 10; i++)
            {
                var run = new Run();
            }

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    TestEvent.Instance.Publish();
                    TestEventA.Instance.Publish("test txt");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    class TestEvent : WeakReferencePubSubEvent
    {
        public static TestEvent Instance { get; } = new TestEvent();

        TestEvent()
        {
        }
    }

    class TestEventA : WeakReferencePubSubEvent<string>
    {
        public static TestEventA Instance { get; } = new TestEventA();

        TestEventA()
        {
        }
    }

    class Run
    {
        public Run()
        {
            TestEvent.Instance.Subscribe(this,
                () => { $@"Run TestEvent Subscribe".WriteLine(); });
            TestEventA.Instance.Subscribe(this,
                s => { $@"Run TestEventA {s} Subscribe".WriteLine(); });
        }

        ~Run()
        {
            $@"~Run".WriteLine(ConsoleColor.Yellow);
        }
    }

    public static class ConsoleExtensions
    {
        public static void WriteLine<T>(this T t,
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            ConsoleColor backgroundBuff = Console.BackgroundColor;
            ConsoleColor foregroundBuff = Console.ForegroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(t);
            Console.BackgroundColor = backgroundBuff;
            Console.ForegroundColor = foregroundBuff;
        }

        public static void ConsoleSplitLine(char splitLineChar = '_',
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            int width = Console.WindowWidth;
            new string(splitLineChar, width - 1).WriteLine(foregroundColor,
                backgroundColor);
        }
    }
}