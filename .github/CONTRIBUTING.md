# Contributing

If you are submitting a fix for a non-trivial bug, please include a full
description of the bug and how to reproduce it, as well as an explanation of
your fix.

If your pull request is an addition or change to functionality of Alkahest,
please include a justification for why that functionality should be in Alkahest.
We have a very specific idea of what the Alkahest core library and plugins
should and should not support, mainly because we do not want TERA publishers to
ban the use of Alkahest. So, if the functionality you are adding seems sketchy
(e.g. if it could *potentially* enable botting or even outright cheating), you
will have to convince us that including it is a good idea.

Finally, if your pull request includes changes to packet structures, we expect
you to provide evidence to back up those changes. We will not blindly accept such
changes, as one of the goals of this project is to properly understand the TERA
network protocol. Also, be sure to submit a pull request with your
changes to the [tera-data](https://github.com/tera-toolbox/tera-data) project as
that is the repository where most protocol reverse engineers in the TERA
community store their research.
