using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RisikoOnline.Services
{
    public class RandomService
    {
        private readonly Random _random = new Random();

        public async Task<int> GetInt()
        {
            return await Task.Run(() =>
            {
                lock (_random)
                    return _random.Next();
            });
        }
        
        public async Task<int> GetInt(int max)
        {
            return await Task.Run(() =>
            {
                lock (_random)
                    return _random.Next(max);
            });
        }

        public async Task<TResult> Choose<TResult>(IReadOnlyList<TResult> list)
        {
            return list[await GetInt(list.Count)];
        }
    }
}
