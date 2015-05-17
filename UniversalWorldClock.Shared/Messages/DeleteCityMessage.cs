using Microsoft.Practices.Prism.PubSubEvents;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Messages
{
    public class DeleteCityMessage : PubSubEvent<CityInfo>
    {
    }
}