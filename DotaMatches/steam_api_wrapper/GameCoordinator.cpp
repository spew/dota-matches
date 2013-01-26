#include "stdafx.h"
#include "Externs.h"
#include "opensteamworks\ISteamGameCoordinator001.h"


EGCResults SendMessage( uint32 unMsgType, const void *pubData, uint32 cubData )
{
	EGCResults result = g_pApiContext->SteamGameCoordinator()->SendMessage(unMsgType, pubData, cubData);
	return result;
}

bool IsMessageAvailable( uint32 *pcubMsgSize )
{
	bool result = g_pApiContext->SteamGameCoordinator()->IsMessageAvailable(pcubMsgSize);
	return result;
}

EGCResults RetrieveMessage(uint32 *punMsgType, void *pubDest, uint32 cubDest, uint32 *pcubMsgSize ) 
{
	return g_pApiContext->SteamGameCoordinator()->RetrieveMessage(punMsgType, pubDest, cubDest, pcubMsgSize);
}