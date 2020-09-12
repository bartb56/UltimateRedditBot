using System;
using System.Threading;
using UltimateRedditBot.Core.AppService;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Repository;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.Core.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private Context _dbContext;
        private SubRedditAppService _subReddit;
        private SubRedditHistoryAppService _subRedditHistory;
        private PostAppService _post;
        private GuildSettingsAppService _guildSettings;
        private GuildAppService _guild;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public UnitOfWork(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public ISubRedditAppService SubRedditRepository
        {
            get
            {
                return _subReddit ??
                     (_subReddit = new SubRedditAppService(_dbContext));
            }
        }

        public ISubRedditHistoryAppService SubRedditHistoryRepository
        {
            get
            {
                return _subRedditHistory ??
                    (_subRedditHistory = new SubRedditHistoryAppService(_dbContext));
            }
        }


        public IPostAppService PostRepository
        {
            get
            {
                return _post ??
                    (_post = new PostAppService(_dbContext));
            }
        }

        public IGuildAppService GuildRepository
        {
            get
            {
                return _guild ??
                    (_guild = new GuildAppService(_dbContext));
            }
        }

        public IGuildSettingsAppService GuildSettingsRepository
        {
            get
            {
                return _guildSettings ??
                   (_guildSettings = new GuildSettingsAppService(_dbContext, GuildRepository));
            }
        }

        public void Commit()
        {
            semaphoreSlim.WaitAsync();
            try
            {
                _dbContext.SaveChanges();
            }
            catch(Exception e)
            {
                semaphoreSlim.Release();
            }
            finally
            {
                semaphoreSlim.Release();
            }
            
        }
    }
}
