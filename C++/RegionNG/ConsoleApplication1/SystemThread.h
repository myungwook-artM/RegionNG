#pragma once

#include "MessageWorker.h"


// Singleton LogicThread
class LogicThread : public MessageWorkerPack {
public:
    static LogicThread& Instance() {
        static LogicThread instance;
        return instance;
    }
private:
    LogicThread() : MessageWorkerPack(4) {}

};

// Singleton DBThread
class DBThread : public MessageWorkerPack {
public:
    static DBThread& Instance() {
        static DBThread instance;
        return instance;
    }

private:
    DBThread() : MessageWorkerPack(4) {}
};


// Singleton PacketThread
class PacketThread : public MessageWorkerPack {
public:
    static PacketThread& Instance() {
        static PacketThread instance;
        return instance;
    }

private:
    PacketThread() : MessageWorkerPack(4) {}
};

