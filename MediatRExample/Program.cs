using Autofac;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Entry>().AsSelf();

            // this will add all your Request- and Notificationhandler
            // that are located in the same project as your program-class
            builder.RegisterMediatR(typeof(Program).Assembly);

            var container = builder.Build();

            var entry = container.Resolve<Entry>();

            await entry.SayHello();

            Console.ReadLine();
        }
    }

    public class Entry
    {
        private readonly IMediator _mediator;

        public Entry(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SayHello()
        {
            var txt = await _mediator.Send(new HelloCommand("Kenneth"));
            await _mediator.Publish(new HelloReceived(txt));
        }
    }

    public record HelloCommand(string Name) : IRequest<string>;

    //public class HelloCommand : IRequest<string>
    //{
    //    public string Name { get; init; }
    //    public HelloCommand(string name)
    //    {
    //        Name = name;
    //    }
    //}


    public class HelloCommandHandler : IRequestHandler<HelloCommand, string>
    {
        public Task<string> Handle(HelloCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"hello {request.Name}");
        }
    }

    public record HelloReceived(string FullText) : INotification;

    public class HelloReceivedListener : INotificationHandler<HelloReceived>
    {
        public Task Handle(HelloReceived notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(HelloReceivedListener)} : received : {notification.FullText}");
            return Task.CompletedTask;
        }
    }

    public class HelloReceivedListener2 : INotificationHandler<HelloReceived>
    {
        public Task Handle(HelloReceived notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(HelloReceivedListener2)} : received : {notification.FullText}");
            return Task.CompletedTask;
        }
    }
}
