using GeniusStoreERP.Application.Dtos.ListItemDto;
using GeniusStoreERP.UI.Common;
using System.Collections.ObjectModel;

namespace GeniusStoreERP.UI.ViewModels.Transactions;

public class InvoiceListViewModel : BaseViewModel
{


    public ObservableCollection<InvoiceListItemDto> InvoiceList { get; set; } = new ObservableCollection<InvoiceListItemDto>();

}
