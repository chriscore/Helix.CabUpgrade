# Helix.CabUpgrade
A tool to upgrade legacy cab blocks in Line 6 Helix patches built before Firmware 3.50 to new Helix cabs

This repo contains is a .NET 6.0 site written in C# using Razor and a C# class library with functions to manipulate Helix preset files. 
The site has a Dockerfile, so can run in docker, and it is currently hosted in Azure (below).

It currently helps automate upgrading legacy cabs in helix preset hlx files to the new ones. It's a solo side project so it's not very polished, but I'd be interested to hear any feedback or any ideas for more capabilities if people have any.

The site can be used here: https://helixcabupgrade.azurewebsites.net/
