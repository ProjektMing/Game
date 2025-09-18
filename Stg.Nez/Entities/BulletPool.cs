using System.Collections.Concurrent;

namespace Stg.Nez.Entities;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Modified from Microsoft.Extensions.ObjectPool.DefaultObjectPool.cs

public class BulletPool(int maximumRetained)
{
    private readonly int _maxCapacity = maximumRetained - 1;
    private int _numItems;

    private protected readonly ConcurrentQueue<Bullet> _items = new();
    private protected Bullet? _fastItem;

    public BulletPool()
        : this(Environment.ProcessorCount * 2) { }

    /// <inheritdoc />
    public Bullet Get()
    {
        var item = _fastItem;
        if (item == null || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
        {
            if (_items.TryDequeue(out item))
            {
                Interlocked.Decrement(ref _numItems);
                return item;
            }

            // no object available, so go get a brand new one
            return Create();
        }

        return item;
    }

    public void Return(Bullet obj)
    {
        ReturnCore(obj);
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    /// <returns>true if the object was returned to the pool</returns>
    private protected bool ReturnCore(Bullet obj)
    {
        if (!ReturnOne(obj))
        {
            // policy says to drop this object
            return false;
        }

        if (_fastItem != null || Interlocked.CompareExchange(ref _fastItem, obj, null) != null)
        {
            if (Interlocked.Increment(ref _numItems) <= _maxCapacity)
            {
                _items.Enqueue(obj);
                return true;
            }

            // no room, clean up the count and drop the object on the floor
            Interlocked.Decrement(ref _numItems);
            return false;
        }

        return true;
    }

    #region Policy
    private Bullet Create()
    {
        return new Bullet(this);
        throw new NotImplementedException(
            "Create method must be implemented to instantiate Bullet objects."
        );
    }

    private static bool ReturnOne(Bullet obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(
                nameof(obj),
                "Cannot return a null object to the pool."
            );
        }
        obj.Reset();
        return true;
    }
    #endregion
}
