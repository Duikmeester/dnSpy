﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using dnSpy.Contracts.Command;
using dnSpy.Contracts.Text.Editor;
using dnSpy.Scripting.Roslyn.Commands;
using Microsoft.VisualStudio.Text.Editor;

namespace dnSpy.Scripting.Roslyn.Common {
	[ExportCommandTargetFilterProvider(RoslynReplCommandConstants.CMDTARGETFILTER_ORDER_ROSLYN_REPL)]
	sealed class RoslynReplCommandTargetFilterProvider : ICommandTargetFilterProvider {
		public ICommandTargetFilter Create(object target) {
			var textView = target as ITextView;
			if (textView?.Roles.Contains(PredefinedDnSpyTextViewRoles.RoslynRepl) != true)
				return null;

			return new RoslynReplCommandTargetFilter(textView);
		}
	}
}
