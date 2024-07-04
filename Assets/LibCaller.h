#ifndef FUNCTONS_H
#define FUNCTONS_H
#include <string>

extern "C" unsigned char Check_sum(unsigned char* r_d, unsigned short Len);
extern "C" unsigned char find_crc(unsigned char out_crc);

extern "C" void send_controllight(wchar_t* targetIP, int targetPort);
#endif