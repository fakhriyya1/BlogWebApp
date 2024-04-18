namespace BlogCK.Web.ResultMessages
{
    public static class Messages
    {
        public static class Article
        {
            public static string Add(string articleTitle)
            {
                return $"'{articleTitle}' article is successfully added.";
            }

            public static string Update(string articleTitle)
            {
                return $"'{articleTitle}' article is successfully updated.";
            }
        }

        public static class Category
        {
            public static string Add(string categoryName)
            {
                return $"'{categoryName}' category name is successfully added.";
            }

            public static string Update(string categoryName)
            {
                return $"'{categoryName}' category name is successfully updated.";
            }
        }

        public static class User
        {
            public static string Add(string userName)
            {
                return $"{userName} named user is successfully added.";
            }

            public static string Update(string userName)
            {
                return $"{userName} named user is successfully updated.";
            }

            public static string Delete(string userName)
            {
                return $"{userName} named user is successfully deleted.";
            }
        }
    }
}
