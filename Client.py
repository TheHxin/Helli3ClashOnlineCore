import socket
import sys

_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ("127.0.0.1",38532)
_socket.connect(server_address)

while True:
    msg = input("Enter Message: ")
    if msg == "exit":
        _socket.close()
        break
    buffer = bytes(msg,"ascii")
    _socket.sendall(buffer)