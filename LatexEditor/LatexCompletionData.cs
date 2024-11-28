using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor
{
    public class LatexCompletionData : ICompletionData
    {
        public LatexCompletionData(string text)
        {
            Text = text;
        }
        public IImage Image => null;

        public string Text { get; }

        public object Content => contentControl ??= BuildContentControl();

        public object Description => $"Description of {Text}";

        public double Priority { get; } = 0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        Control BuildContentControl()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = Text;
            textBlock.Margin = new Thickness(5);

            return textBlock;
        }

        Control contentControl;
    }
}
