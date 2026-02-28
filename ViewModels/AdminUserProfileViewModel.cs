namespace StoreWave.ViewModels
{
    /// <summary>
    /// ViewModel for displaying a single user's profile in the Admin area
    /// </summary>
    public class AdminUserProfileViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public int TotalOrders { get; set; }

        /// <summary>
        /// Returns the user's initials for avatar display
        /// </summary>
        public string Initials => $"{(FirstName.Length > 0 ? FirstName[0] : '?')}{(LastName.Length > 0 ? LastName[0] : '?')}".ToUpper();
    }

    /// <summary>
    /// Wrapper ViewModel for the Admin Users list page with search/filter support
    /// </summary>
    public class AdminUsersListViewModel
    {
        public List<AdminUserProfileViewModel> Users { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public List<string> AvailableRoles { get; set; } = new();
        public int TotalUsers => Users.Count;
        public int ActiveUsers => Users.Count(u => u.IsActive);
        public int InactiveUsers => Users.Count(u => !u.IsActive);
    }
}
