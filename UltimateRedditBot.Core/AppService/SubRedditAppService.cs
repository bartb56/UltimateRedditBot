using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class SubRedditAppService : Repository<SubReddit>, ISubRedditAppService
    {
        public SubRedditAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {
        }

        public override async Task<IAsyncEnumerable<SubReddit>> GetAll()
        {
            return Queryable().Include(x => x.SubRedditHistories).ThenInclude(x => x.LastPost).ToAsyncEnumerable();
        }

        public override async Task<SubReddit> GetById(int id)
        {
            return await Queryable().Include(x => x.SubRedditHistories).ThenInclude(x => x.LastPost).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var all = Queryable().Include(x => x.SubRedditHistories);
            if (!await all.AnyAsync())
                return null;

            return await all.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
