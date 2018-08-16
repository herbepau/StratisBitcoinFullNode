﻿using System;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Features.SignalR
{
    public interface ISignalRService
    {
        string ChannelPrefix { get; }

        Task PublishAsync(string topic, string data);

        IObservable<(string topic, string data)> MessageStream { get; }
    }
}
