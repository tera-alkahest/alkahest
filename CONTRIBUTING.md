# Contributing

If you're submitting a fix for a non-trivial bug, please include a full
description of the bug and how to reproduce it, as well as an explanation of
your fix.

If your pull request is an addition or change to functionality of Alkahest,
please include a justification for why that functionality should be in
Alkahest. I have a very specific idea of what the Alkahest core library and
plugins should and shouldn't support, mainly because I don't want TERA
publishers to ban the use of Alkahest. So, if the functionality you're adding
seems sketchy (e.g. if it could *potentially* enable botting or even outright
cheating), you're gonna need to convince me that including it is a good idea.

Finally, if your pull request includes changes to packet structures, I do
expect you to provide evidence to back up those changes. I will not blindly
accept such changes, as one of the goals of this project is to properly
understand the TERA protocol. Also, be sure to submit a pull request with your
changes to the [tera-data](https://github.com/meishuu/tera-data) project as
that's the repository where most protocol reverse engineers in the TERA
community store their research.
