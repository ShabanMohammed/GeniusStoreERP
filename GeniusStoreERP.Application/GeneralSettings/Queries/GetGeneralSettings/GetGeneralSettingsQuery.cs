using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;

public record GetGeneralSettingsQuery() : IRequest<GeneralSettingsDto?>;
