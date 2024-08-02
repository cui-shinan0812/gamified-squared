// BoolArrayDLL.h
#ifdef BOOLARRAYDLL_EXPORTS
#define BOOLARRAYDLL_API __declspec(dllexport)
#else
#define BOOLARRAYDLL_API __declspec(dllimport)
#endif

extern "C" {
    BOOLARRAYDLL_API bool** GetBoolArray(int& rows, int& cols);
}