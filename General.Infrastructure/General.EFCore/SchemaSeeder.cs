using General.Infrastructure.EFCore.EntityContext;
using Microsoft.EntityFrameworkCore;

namespace General.Infrastructure.EFCore
{
	public class SchemaSeeder
    {
        public void Execute(RowEntityContext context)
        {

            string dropProcedure = @"IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[SP_USERLIST]')) BEGIN  DROP PROCEDURE SP_USERLIST END";



            string createProc = @"
  
  
CREATE PROCEDURE SP_USERLIST     
    
 @SortBy NVARCHAR(250)=NULL   ,     
 @Search NVARCHAR(250)=NULL     ,    
 @Count INT =NULL       ,    
 @StartRowIndex INT =0      ,    
 @MaximumRows INT =100      ,    
 @RoleIds NVARCHAR(MAX) = null        
    
AS    
    
    
SELECT     
u.Email, u.UserName ,u.PhoneNumber,    
P.* , U.EmailConfirmed,      
dbo.FUNC_GETROLENAMES(u.Id) RoleNames,    
'' RoleIds    
inTO #tmp     
FROM UserProfile  p WITH(NOLOCK)    
INNER JOIN AspNetUsers u WITH(NOLOCK)  ON p.AppUserId = u.Id               
    
WHERE p.Deleted=0             
AND (@RoleIds IS NULL OR (    
AppUserId IN (SELECT UserID From AspnetuserRoles WITH(NOLOCK) WHERE RoleId in(SELECT [Value] FROM  dbo.FUNC_SPLIT(@RoleIds)))   

)    
)    
AND               
 (@Search Is NULL               
 OR (u.Email LIKE '%'+@Search+'%')               
 OR (p.FirstName LIKE '%'+@Search+'%')               
 OR (p.LastName LIKE '%'+@Search+'%')                   
 OR (u.phoneNumber LIKE '%'+@Search+'%')                         
 )     
 SET @Count = (SELECT COUNT(1) FROM #tmp)              
;WITH tbl AS(                              
SELECT                               
  ROW_NUMBER() OVER (ORDER BY                              
        CASE WHEN @SortBy = '' OR @SortBy IS NULL  THEN CreatedOn END DESC,                              
  CASE WHEN @SortBy = 'Name asc' THEN  firstName + lastName END ASC,                              
  CASE WHEN @SortBy = 'Name desc' THEN firstName + lastName END DESC,                              
                              
  CASE WHEN @SortBy = 'LastName asc' THEN firstName END ASC,                              
  CASE WHEN @SortBy = 'LastName desc' THEN firstName END DESC,          
      
  CASE WHEN @SortBy = 'Email asc' THEN email END ASC,                              
  CASE WHEN @SortBy = 'Email desc' THEN email END DESC,    
      
  CASE WHEN @SortBy = 'PhoneNumber asc' THEN PhoneNumber END ASC,                              
  CASE WHEN @SortBy = 'PhoneNumber desc' THEN PhoneNumber END DESC,  
  
    CASE WHEN @SortBy = 'CreatedOn asc' THEN CreatedOn END ASC,                              
    CASE WHEN @SortBy = 'CreatedOn desc' THEN CreatedOn END DESC,  
  
  CASE WHEN @SortBy = 'ModifiedOn asc' THEN ModifiedOn END ASC,                              
    CASE WHEN @SortBy = 'ModifiedOn desc' THEN ModifiedOn END DESC  
                              
) AS ROWNUM,     
#tmp.* FROM  #tmp    
    
    
      
)      
              
    
SELECT @Count AS TotalCount , * FROM tbl                               
WHERE RowNum>@StartRowIndex AND RowNum<=(@StartRowIndex+@MaximumRows)                              
ORDER BY ROWNUM    
DROP TABLE   #tmp           
";


            string dropFunc = @"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FUNC_SPLIT]')) BEGIN  DROP FUNCTION FUNC_SPLIT END";
            string createFunc = @"
CREATE FUNCTION FUNC_SPLIT  
(   
 -- Add the parameters for the function here  
 @stringValue NVARCHAR(MAX)  
)  
RETURNS TABLE   
AS  
RETURN   
(  
SELECT [value]  As [value]  
FROM STRING_SPLIT(@stringValue, ',')  
)  
";
            string dropFunc1 = @"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FUNC_GETROLENAMES]')) BEGIN  DROP FUNCTION FUNC_GETROLENAMES END";

            string createFunc1 = @"
CREATE FUNCTION FUNC_GETROLENAMES  
(  
    -- Add the parameters for the function here  
   @UserId NVARCHAR(500)  
)  
RETURNS NVARCHAR(MAX)  
AS  
BEGIN  
    -- Declare the return variable here  
 DECLARE @RoleNames NVARCHAR(MAX)  
   SELECT @RoleNames = STRING_AGG(AspNetRoles.Name, ', ')    
FROM AspNetRoles  
LEFT OUTER JOIN AspNetUserRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId  
WHERE UserId=@UserId  
  
    -- Return the result of the function  
    RETURN @RoleNames  
END  ";

            context.Database.ExecuteSqlRaw(dropFunc);
            context.Database.ExecuteSqlRaw(createFunc);
            context.Database.ExecuteSqlRaw(dropFunc1);
            context.Database.ExecuteSqlRaw(createFunc1);
            context.Database.ExecuteSqlRaw(dropProcedure);
            context.Database.ExecuteSqlRaw(createProc);
        }
    }
}
