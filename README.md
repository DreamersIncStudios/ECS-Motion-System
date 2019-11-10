# ECS-Motion-System
__Copyright 2019, Dreamers Inc Studios__
  

This project is a recreation and adaption of the Unity Standard Assets Third Person Character Controllor using Unity DOTS Framework. In my initial testing, I have found that running roughly 500 of the MonoBehaviour Character controllers in a scene drops performance down to 60FPS at 800 by 600 at medium quality settings running on a 1800X OC 4.0ghz with RX5700XT. 
The ultimate goal would be to get the same number of characters running using ECS at twice the framerate. 

This system is being designed to work with the ECS IAUS repo as they both will be combined as part of the AI system for an open world action RPG in the works. In addition to porting the controller to DOTS, We are also adding a character swap system and a object culling/deactivation system based on proximity. 


Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
