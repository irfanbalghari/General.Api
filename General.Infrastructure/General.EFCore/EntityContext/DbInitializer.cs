namespace General.Infrastructure.EFCore.EntityContext
{
	public static class DbInitializer
	{
		private static void Initialize(GeneralEntityContext context)
		{
			context.Database.EnsureCreated();
		}
	}
}
