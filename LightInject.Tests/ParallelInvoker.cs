namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A helper class to verify that a given method does not break 
    /// when executed on multiple threads.
    /// </summary>
    public static class ParallelInvoker
    {
        /// <summary>
        /// Invokes the <paramref name="action"/> in parallel using the given number of <see cref="Task"/> instances.
        /// </summary>
        /// <param name="taskCount">The number of <see cref="Task"/> instances to be used.</param>
        /// <param name="action">The <see cref="Action"/> to be executed.</param>
        public static void Invoke(int taskCount, Action action)
        {
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = new Task(action);
            }

            for (int i = 0; i < taskCount; i++)
            {
                tasks[i].Start();
            }

            for (int i = 0; i < taskCount; i++)
            {
                tasks[i].Wait();
            }
        }

        public static void Invoke(int taskCount, params Action[] actions)
        {
            int totalTaskCount = taskCount * actions.Length;
            int actionCount = actions.Length;
            int taskIndex = 0;
            var tasks = new Task[totalTaskCount];

            for (int i = 0; i < actionCount; i++)
            {
                for (int j = 0; j < taskCount; j++)
                {
                    tasks[taskIndex] = new Task(actions[i]);
                    taskIndex++;
                }
            }

            var startIndicies = GetRandomStartIndicies(totalTaskCount);

            for (int i = 0; i < totalTaskCount; i++)
            {
                tasks[startIndicies[i]].Start();
            }


            for (int i = 0; i < totalTaskCount; i++)
            {
                tasks[i].Wait();
            }
        }

        private static int[] GetRandomStartIndicies(int count)
        {
            var result = new List<int>(count);

            Random random = new Random();
            while (result.Count < count)
            {
                var value = random.Next(0, count);
                if (!result.Contains(value))
                {
                    result.Add(value);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Invokes the <paramref name="function"/> in parallel using the given number of <see cref="Task"/> instances.
        /// </summary>
        /// <typeparam name="T">The type returned from the <paramref name="function"/> delegate.</typeparam>
        /// <param name="taskCount">The number of <see cref="Task"/> instances to be used.</param>
        /// <param name="function">The <see cref="Func{TResult}"/> to be executed.</param>
        /// <param name="callback">An <see cref="Action{T}"/> callback delegate that can be used 
        /// to collect the result from invoking the <paramref name="function"/> delegate.</param>
        public static void Invoke<T>(int taskCount, Func<T> function, Action<T> callback)
        {
            Invoke(taskCount, () => callback(function()));
        }
    }
}
