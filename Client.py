import socket
import sys

_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ("127.0.0.1",38532)
_socket.connect(server_address)

msg = "hi i am client"
buffer = bytes(msg,"ascii")
_socket.sendall(buffer)

input()