#pragma once

#include <Windows.h>

#define DllExport extern "C" __declspec(dllexport) 

DllExport void __stdcall Run(const wchar_t* args);