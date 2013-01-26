
#include "opensteamworks\SteamTypes.h"
#include "opensteamworks\GameCoordinatorCommon.h"

void Shutdown();
bool Initialize();

EGCResults SendMessage( uint32 unMsgType, const void *pubData, uint32 cubData );

bool IsMessageAvailable( uint32 *pcubMsgSize );

EGCResults RetrieveMessage(uint32 *punMsgType, void *pubDest, uint32 cubDest, uint32 *pcubMsgSize ) ;