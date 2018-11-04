#include "Bootstrap.h"

DllExport void __stdcall Run(const wchar_t* args)
{
	const DWORD pid = GetCurrentProcessId();
	wchar_t buf[64];
	wsprintf(buf, L"Current Process: %d Args: %ls", pid, args);

	MessageBox(nullptr, buf, L"LibraryPlusPlus", NULL);
}