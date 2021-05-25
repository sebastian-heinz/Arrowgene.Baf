Arrowgene.Baf
===
Research for reviving and server emulation of the game BurstAFever

## Table of contents
- [Disclaimer](#disclaimer)

Disclaimer
===
The project is intended for educational purpose only, we strongly discourage operating a public server.
This repository does not distribute any game related assets or copyrighted material,
pull requests containing such material will not be accepted.
All files have been created from scratch, and are subject to the copyright notice of this project.
If a part of this repository violates any copyright or license, please state which files and why,
including proof and it will be removed.

## Starting the game
```
start baf.bin 1 a b c:d 127.0.0.1 3232 arrowgene.net
```

## Assets

Datas.sai
- lookup table

| bytes                     | type             | description                      |  
| ------------------------- |------------------|----------------------------------| 
| 4 bytes                   | c-string         | magic bytes "SDO"                |
| 4 bytes                   | int              | number of attribute blocks       |
| 4 bytes                   | int              | size in bytes of file name block |
| 4 bytes                   | int              | unknown                          |
| 16 * num of entries bytes | attribute blocks | a block consists of 16 bytes     |
| file name block bytes     | c-strings        |                                  |


Datas.sac
- raw data