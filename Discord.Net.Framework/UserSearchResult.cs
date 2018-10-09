using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Net.Framework
{
    public class UserSearchResult
    {


        public UserSearchResult(UserSearchResultStatus status, IUser match = null)
        {
            Status = status;
            if (status == UserSearchResultStatus.FoundMatch)
                Success = true;
            Match = match;
        }

        public UserSearchResultStatus Status { get; set; }
        public bool Success { get; set; }
        public IUser Match { get; set; }

        public enum UserSearchResultStatus { FoundMatch, NoMatch, MultipleMatches }
    }
}
