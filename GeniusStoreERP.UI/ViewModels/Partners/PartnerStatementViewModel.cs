using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerStatement;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;

namespace GeniusStoreERP.UI.ViewModels.Partners
{
    public class PartnerStatementViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly INavigationService _navigationService;
        private int _partnerId;

        private DateTime? _fromDate = DateTime.Now.AddMonths(-1);
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (SetProperty(ref _fromDate, value))
                {
                    _ = LoadStatementAsync();
                }
            }
        }

        private DateTime? _toDate = DateTime.Now;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (SetProperty(ref _toDate, value))
                {
                    _ = LoadStatementAsync();
                }
            }
        }

        private PartnerStatementDto? _statement;
        public PartnerStatementDto? Statement
        {
            get => _statement;
            set => SetProperty(ref _statement, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand GoBackCommand { get; }

        public PartnerStatementViewModel(IMediator mediator, INavigationService navigationService)
        {
            _mediator = mediator;
            _navigationService = navigationService;

            RefreshCommand = new RelayCommand(_ => _ = LoadStatementAsync());
            PrintCommand = new RelayCommand(_ => OnPrint());
            GoBackCommand = new RelayCommand(_ => _navigationService.NavigateTo<PartnerAccountsViewModel>());
        }

        public override async void Initialize(object? parameter)
        {
            if (parameter is int partnerId)
            {
                _partnerId = partnerId;
                await LoadStatementAsync();
            }
        }

        private async Task LoadStatementAsync()
        {
            if (_partnerId == 0) return;

            IsLoading = true;
            try
            {
                var query = new GetPartnerStatementQuery(
                    _partnerId,
                    FromDate,
                    ToDate
                );

                Statement = await _mediator.Send(query);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"خطأ في تحميل كشف الحساب: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnPrint()
        {
            MessageBoxService.ShowInfo("دالة الطباعة قيد التطوير حالياً");
        }
    }
}
