using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example.Security.Core
{
    public interface ISecurityProvider
    {
        bool IsAuthenticated { get; }
        string? UserName { get; }
        Task<string?> Login(string[] scopes, bool forceInteractive = false, CancellationToken token = default);
        Task Logout(CancellationToken token = default);
        Task<string?> GetAccessToken(string[] scopes, CancellationToken token = default);
    }
}
