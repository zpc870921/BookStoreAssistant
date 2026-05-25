using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.AI;
using System.Linq.Expressions;

namespace bookstoreagent.Infrastructure.Converters
{
    public class ChatRoleConverter(): ValueConverter<ChatRole, string>(r=>r.Value,r=>new ChatRole(r))
    {

    }
}
