using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace EmitAwaitDemo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var getV = typeof(Program).GetMethod(nameof(Program.GetV));

            // AddSixUseContinueWith
            await Test(getV, typeof(Program).GetMethod(nameof(Program.AddSixUseContinueWith)));

            // AddSixUseAwaiter
            await Test(getV, typeof(Program).GetMethod(nameof(Program.AddSixUseAwaiter)));

            // AddSixUseAsyncAwait
            await Test(getV, typeof(Program).GetMethod(nameof(Program.AddSixUseAsyncAwait)));
        }

        private static async Task Test(MethodInfo method, MethodInfo awaitMehtod)
        {
            var caller = CreateCaller(method, awaitMehtod);
            Console.WriteLine($"Start {awaitMehtod.Name} at: {DateTime.Now}.");
            var task = caller();
            Console.WriteLine($"Call done at: {DateTime.Now}.");
            var number = await task;
            Console.WriteLine($"Hello {number} at: {DateTime.Now}.");
            Console.WriteLine($"End at: {DateTime.Now}.");
            Console.WriteLine();
        }

        public static Func<Task<int>> CreateCaller(MethodInfo method, MethodInfo awaitMehtod)
        {
            var m = new DynamicMethod(Guid.NewGuid().ToString("N"), typeof(Task<int>), Type.EmptyTypes);
            var il = m.GetILGenerator();
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Call, awaitMehtod);
            il.Emit(OpCodes.Ret);

            return m.CreateDelegate(typeof(Func<Task<int>>)) as Func<Task<int>>;
        }

        public static async Task<int> GetV()
        {
            await Task.Delay(2000);
            return 55;
        }

        public static async Task<int> AddSixUseAsyncAwait(Task<int> task)
        {
            var r = await task;
            Console.WriteLine($"AddSixUseAsyncAwait is: {DateTime.Now}.");
            return r + 6;
        }

        public static Task<int> AddSixUseContinueWith(Task<int> task)
        {
            return task.ContinueWith(i =>
            {
                Console.WriteLine($"AddSixUseContinueWith is: {DateTime.Now}.");
                return i.Result + 6;
            });
        }

        public static Task<int> AddSixUseAwaiter(Task<int> task)
        {
            var r = task.ConfigureAwait(false).GetAwaiter().GetResult() + 6;
            Console.WriteLine($"AddSixUseAwaiter is: {DateTime.Now}.");
            return Task.FromResult(r);
        }
    }
}