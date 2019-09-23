namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class BaseRepository
    {
        protected readonly ApplicationDbContext context;

        public BaseRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

    }
}
