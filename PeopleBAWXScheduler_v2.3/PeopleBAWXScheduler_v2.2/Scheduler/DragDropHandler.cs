using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using SimplestDragDrop;
using System.Windows.Media.Animation;

namespace Scheduler
{

    
    class DragDropHandler
    {

        //Flag to denote if dragging or not
        private bool _isDragging;
        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        }

        Point _startPoint;

        public void g_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _startPoint = e.GetPosition(null);

        }

        public void g_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid g = (Grid)sender;
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);
                StartDragGrid(sender, e);
            }
        }

        private void StartDragGrid(object sender, MouseButtonEventArgs e) {
            Grid g = (Grid)sender;
            DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler;
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            _adorner = new DragAdorner(DragScope, (UIElement)g, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);

            IsDragging = true;
            _dragHasLeftScope = false;

            //NEEEEEEEEEEEEEEEEEEEEEEEEEED TO CHANGE
            TextBlock name = (TextBlock) g.Children[0];
            TextBlock duration = (TextBlock) g.Children[1];
            TextBlock path = (TextBlock)g.Children[2];
            String content = g.Tag.ToString() + "#" + path.Text + "#" + duration.Text;
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), content);
            DragDropEffects de = DragDrop.DoDragDrop(g, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            IsDragging = false;
        }

        public void header_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _startPoint = e.GetPosition(null);

        }

        public void header_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);
                StartDragTextBlock(sender, e);
            }
        }

        private void StartDragTextBlock(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender; 
            DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler;
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            _adorner = new DragAdorner(DragScope, (UIElement)tb, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);

            IsDragging = true;
            _dragHasLeftScope = false;

            String content = tb.Tag.ToString();
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), content);
            DragDropEffects de = DragDrop.DoDragDrop(tb, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            IsDragging = false;
        }

        public void dataGrid1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        public DataGridRow dg = new DataGridRow();
        public TreeViewItem tvi = new TreeViewItem();
        public void dataGrid1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DataGridRow a = (DataGridRow)sender;
            dg = a;
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);
                StartDragDataGrid(sender, e);
            }
        }

        public void subitem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        public void subitem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            TreeViewItem a = (TreeViewItem)sender;
            tvi = a;
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);
                StartDragSubItem(sender, e);
            }
        }

        private void StartDragSubItem(object sender, MouseEventArgs e)
        {
            TreeViewItem tvi2 = (TreeViewItem)sender;


            DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler;
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            _adorner = new DragAdorner(DragScope, (UIElement)this.tvi, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false;

            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), tvi2.Tag);
            DragDropEffects de = DragDrop.DoDragDrop(tvi2, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            IsDragging = false;
        }


        FrameworkElement _dragScope;
        public FrameworkElement DragScope
        {
            get { return _dragScope; }
            set { _dragScope = value; }
        }

        private bool _dragHasLeftScope = false;
        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }
        }

        void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }
        }

        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }

        public void StartDragDataGrid(object sender, MouseEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;

            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            DragScope = Application.Current.MainWindow.Content as FrameworkElement;

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler;

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            //Here we create our adorner.. 
            _adorner = new DragAdorner(DragScope, (UIElement)this.dg, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false;
            //Finally lets drag drop 
            string content = "dgr" + "#" + dgr.Tag.ToString();
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), content);
            DragDropEffects de = DragDrop.DoDragDrop(dgr, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            IsDragging = false;
        }

        DragAdorner _adorner = null;
        AdornerLayer _layer;


    }
}


