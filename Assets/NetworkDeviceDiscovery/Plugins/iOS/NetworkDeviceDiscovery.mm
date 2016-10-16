
#include <ifaddrs.h>
#include <arpa/inet.h>

char* ndd_cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}


static NSString *deviceIPAddress ()
{
    NSString *address = @"error";
    struct ifaddrs *interfaces = NULL;
    struct ifaddrs *temp_addr = NULL;
    int success = 0;
    
#if TARGET_IPHONE_SIMULATOR
    NSString *networkInterface = @"en1";
#else
    NSString *networkInterface = @"en0";
#endif
    
    // retrieve the current interfaces - returns 0 on success
    success = getifaddrs(&interfaces);
    if (success == 0) {
        // Loop through linked list of interfaces
        temp_addr = interfaces;
        while (temp_addr != NULL) {
            if( temp_addr->ifa_addr->sa_family == AF_INET) {
                // Check if interface is en0 which is the wifi connection on the iPhone
                if ([[NSString stringWithUTF8String:temp_addr->ifa_name] isEqualToString:networkInterface]) {
                    // Get NSString from C String
                    address = [NSString stringWithUTF8String:inet_ntoa(((struct sockaddr_in *)temp_addr->ifa_addr)->sin_addr)];
                }
            }
            
            temp_addr = temp_addr->ifa_next;
        }
    }
    
    // Free memory
    freeifaddrs(interfaces);
    
    return address;
}

extern "C" {
    char* NDDGetIPAddress() {
        
        NSString* ipAddress = deviceIPAddress();
        
        return ndd_cStringCopy([ipAddress UTF8String]);
    }
}
