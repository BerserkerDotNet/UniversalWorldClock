using System;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace UniversalWorldClock.Services
{
    public class ExtendedFrameNavigationService:FrameNavigationService
    {
        private const string LastNavigationParameterKey = "LastNavigationParameter";
        private readonly IFrameFacade _frame;
        private readonly Func<string, Type> _navigationResolver;
        private readonly ISessionStateService _sessionStateService;
        private Experiences _currentExperience;

        public ExtendedFrameNavigationService(IFrameFacade frame, Func<string, Type> navigationResolver, ISessionStateService sessionStateService) : 
            base(frame, navigationResolver, sessionStateService)
        {
            _frame = frame;
            _navigationResolver = navigationResolver;
            _sessionStateService = sessionStateService;
        }

        public bool Navigate(Experiences destination, object argument)
        {
            var success =  base.Navigate(destination.ToString(), argument);
            if (success)
                _currentExperience = destination;
            return success;
        }
        
        public bool Navigate(Experiences destination)
        {
            return Navigate(destination, null);
        }

        public bool Reload()
        {
            Type pageType = _navigationResolver(_currentExperience.ToString());

            if (pageType == null)
                return false;

            var lastNavigationParameter = _sessionStateService.SessionState.ContainsKey(LastNavigationParameterKey)
                ? _sessionStateService.SessionState[LastNavigationParameterKey]
                : null;

            return _frame.Navigate(pageType, lastNavigationParameter);
        }
    }
}
