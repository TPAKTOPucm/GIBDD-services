namespace FineService.Mediators;

public interface IMediator
{
	Task Publish(object notification);
}
