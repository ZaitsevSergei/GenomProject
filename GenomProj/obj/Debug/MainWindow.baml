   M S B A M L   `   `   `  ����  	GenomProj    GenomProj.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � WindowState$  	Maximized=�5   	    � Title$+ %Визуализация Генома��5   	   $	��350��6%   $	��525��62   .��5      �� $
��Auto=�6   $
��Auto=�66   ��. .��5      � $
��Auto=�6   $��Visible=�6=   ��. .+�5
   
   �� .+�5      �� -   5        $
��menu��5      .+�5      �� -   5        $��Menu1str��5   !    �� Orientation$ 
Horizontal=�61   $��	10,10,0,0q�6J   .+�5      �� -   5         ��Name$ OpenFile��5   !   $��Открыть файл��63   $	��100��6a   �� 5      -   5         ��Name$ FileName��5   "   $�� ��64   $	��354��6<   $��10,0,0,0q�6H   5      �� 5      -   5        $��Menu2str��5      $ 
Horizontal=�6-   $��	10,10,0,0q�6F   .+�5      �� .+�5      �� -   5         ��Name$ Param1��5   $   $��Масштаб��64   �� 5      -   5        $ Param2��5   $   $��Ширина��64   5      �� 5      .+�5      �� -	   5        $ 
ScaleValue��5   &   $��1��6:   $��10,0,0,0q�6C   $��80��6U   {�. $��25��6q   �� 5      -
   5        $ 
WidthValue��5   &   $��1��6:   $��10,0,0,0q�6C   $��80��6U   {�. $��25��6q   5      �� 5      -   5        $ OK_btn��5   !   $
��ОК��61   $��10,0,0,0q�6>   {�.  $��38��6w   5      �� 5      -   5         ��Name$ paint_cb��5      $!��Вывод пикселей��61   $��
10,0,355,0q�6J   $��Bottom=�6^   {�.   d� 	IsChecked$
 TrueY�6�   �� 5      .+�5       �� $ 
Horizontal=�6%   $��10,0,0,0q�6>   .+�5!      �� -   5        $��processType��5!   $   $��ProcessType��67   $
��Bold"�6M   $
��Left=�6_   $	��250��6z   �� 5"      -   5        $ 	pbValueLb��5"   $   $��process��67   $
��Bold"�6I   $��0,0,0,0q�6[   $
��Left=�6l   $	��203��6�   5#      �� 5$      $ 
Horizontal=�6%   .+�5%      � -   5         �Name$ PrgrsBar��5%   *   $	��316��6<   $��20��6H   	 � Minimum$	 0Q�6U   $��6,0,0,0q�6a   $
��Left=�6r   �� 5&      -   5        $��
Cancel_btn��5&   %   $��Отмена��67   $��10,0,0,0q�6a   {�.  6s   �� 5'      -   5        $��Save_btn��5'   %   $��Сохранить��65   $��20,0,0,0q�6`   {�.  6r   �� 5(      -   5        $��img_size��5(   $   $��10,0,0,0q�64   $
��auto��6F   5*      5+      5,      �� 50      -   5        $	��img��50      $
��Left=�6$   $	��100��6?   $��	10,30,0,0q�6L   $	��Top=�6_   $	��100��6w   52      53      54      55      