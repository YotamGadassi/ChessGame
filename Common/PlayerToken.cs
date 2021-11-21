using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PlayerToken
    {
        private Guid token = Guid.NewGuid();

        public bool Equals(PlayerToken other)
        {
            return other.token.Equals(token);
        }

    }
}
