using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class SubRedditAppService : Repository<SubReddit>, ISubRedditAppService
    {
        public SubRedditAppService(Context context)
            : base(context)
        {
        }

        public override async Task<IEnumerable<SubReddit>> GetAll()
        {
            return await Queriable().Include(x => x.SubRedditHistories).ThenInclude(x => x.LastPost).ToListAsync();
        }

        public override async Task<SubReddit> GetById(int id)
        {
            return await Queriable().Include(x => x.SubRedditHistories).ThenInclude(x => x.LastPost).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var all = Queriable().Include(x => x.SubRedditHistories);
            if (!await all.AnyAsync())
                return null;

            return await all.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
