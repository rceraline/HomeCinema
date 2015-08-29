using System;

namespace HomeCinema.Data
{
    public interface IDbFactory : IDisposable
    {
        HomeCinemaContext Init();
    }
}
