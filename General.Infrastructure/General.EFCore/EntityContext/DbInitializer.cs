namespace General.Infrastructure.EFCore.EntityContext
{
	public static class DbInitializer
	{
		private static void Initialize(RowEntityContext context)
		{
			context.Database.EnsureCreated();
		}
	}
}
