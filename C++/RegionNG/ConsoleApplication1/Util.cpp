#include "Util.h"
#include <string>
#include <iostream>
#include <Windows.h>

namespace Util
{
    void ReadLine()
    {
        std::string input;
        std::getline(std::cin, input);
    }

    int GetCurrentThreadId()
    {
        return ::GetCurrentThreadId();
    }

}


